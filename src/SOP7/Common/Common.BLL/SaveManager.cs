using Common.BLL.Models.Request;
using Common.BLL.Models.Response;
using Common.IDAL;
using Common.Model.History;
using SDMS.Model.CCTV;
using SDMS.Model.Spatial;
using SOPManager.Model.Sop.Account;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TeamEditor.Model.Sop.Team;

namespace Common.BLL
{
    public class SaveManager
    {
        /// <summary>
        /// 0:POI, 1:공간정보이름, 2:가벽, 3:SOP편집, 4:현황정보, 5:사용자권한부여
        /// </summary>
        public enum TargetType { POI = 0, EquipzoneName, FakeWall, SOP, StatusInfo, UserAuth, BuildingGroupName, BuildingName }

        /// <summary>
        /// 0:추가, 1:수정, 2:삭제, 3:업로드, 4:다운로드
        /// </summary>
        public enum ActionType { Add = 0, Modify, Move, Delete, Upload, Download }

        /// <summary>
        /// [가벽] 0:크기변경, 1:이동, 2:회전
        /// </summary>
        public enum ModifyType { None = -1, ChangeSize = 0, Move, Rotate }

        private IDataManager m_dataManager = null;
        private ProcessManager m_processManager = null;

        public SaveManager(IDataManager dataManager, ProcessManager processManager)
        {
            this.m_dataManager = dataManager;
            this.m_processManager = processManager;
        }

        /// <summary>
        /// POI 추가
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <param name="facilityType"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_AddPOI(int userID, int zoneID, int facilityType, int sensorID)
        {
            TargetType targetType = TargetType.POI;
            ActionType actionType = ActionType.Add;

            string strHistorycontent = MakeHistoryContentPOI(targetType, actionType, zoneID, facilityType, sensorID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// POI 이동, 수정
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <param name="facilityType"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_ModifyPOI(int userID, int zoneID, int facilityType, int sensorID)
        {
            TargetType targetType = TargetType.POI;
            ActionType actionType = ActionType.Modify;

            string strHistorycontent = MakeHistoryContentPOI(targetType, actionType, zoneID, facilityType, sensorID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// POI 삭제
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <param name="facilityType"></param>
        /// <param name="sensorID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_DeletePOI(int userID, int zoneID, int facilityType, int sensorID)
        {
            TargetType targetType = TargetType.POI;
            ActionType actionType = ActionType.Delete;

            string strHistorycontent = MakeHistoryContentPOI(targetType, actionType, zoneID, facilityType, sensorID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 공간정보 이동, 명칭 변경
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <param name="nEquipzoneID"></param>
        /// <param name="orgEquipzoneName">변경하기 전 명칭, 이동한 경우는 빈값</param>
        /// <returns></returns>
        public bool SaveUserHistory_EquipzoneName(int userID, int zoneID, int nEquipzoneID, string orgEquipzoneName = "")
        {
            TargetType targetType = TargetType.EquipzoneName;
            ActionType actionType = (orgEquipzoneName.Length > 0) ? ActionType.Modify : ActionType.Move;

            string strHistorycontent = MakeHistoryContentEquipzoneName(targetType, actionType, zoneID, orgEquipzoneName, nEquipzoneID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 빌딩 그룹 명칭 이동, 명칭 변경
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nBuildingGroupID"></param>
        /// <param name="orgBuildingGroupName">변경하기 전 명칭, 이동한 경우는 빈값</param>
        /// <returns></returns>
        public bool SaveUserHistory_BuildingGroupName(int userID, int nBuildingGroupID, string orgBuildingGroupName = "")
        {
            TargetType targetType = TargetType.BuildingGroupName;
            ActionType actionType = (orgBuildingGroupName.Length > 0) ? ActionType.Modify : ActionType.Move;

            string strHistorycontent = MakeHistoryContentBuildingGroupName(targetType, actionType, nBuildingGroupID, orgBuildingGroupName);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 빌딩 명칭 이동, 명칭 변경
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nBuildingGroupID"></param>
        /// <param name="orgBuildingName">변경하기 전 명칭, 이동한 경우는 빈값</param>
        /// <returns></returns>
        public bool SaveUserHistory_BuildingName(int userID, int nBuildingID, string orgBuildingName = "")
        {
            TargetType targetType = TargetType.BuildingName;
            ActionType actionType = (orgBuildingName.Length > 0) ? ActionType.Modify : ActionType.Move;

            string strHistorycontent = MakeHistoryContentBuildingName(targetType, actionType, nBuildingID, orgBuildingName);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);

            return result;
        }

        /// <summary>
        /// 가벽 추가
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_AddFakeWall(int userID, int zoneID)
        {
            TargetType targetType = TargetType.FakeWall;
            ActionType actionType = ActionType.Add;

            string strHistorycontent = MakeHistoryContentFakeWall(targetType, actionType, ModifyType.None, zoneID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 가벽 이동
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <param name="modifyType">0:크기변경, 1:이동, 2:회전</param>
        /// <returns></returns>
        public bool SaveUserHistory_ModifyFakeWall(int userID, int zoneID, ModifyType modifyType)
        {
            TargetType targetType = TargetType.FakeWall;
            ActionType actionType = (modifyType == ModifyType.None) ? ActionType.Move : ActionType.Modify;

            string strHistorycontent = MakeHistoryContentFakeWall(targetType, actionType, modifyType, zoneID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 가벽 삭제
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="zoneID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_DeleteFakeWall(int userID, int zoneID)
        {
            TargetType targetType = TargetType.FakeWall;
            ActionType actionType = ActionType.Delete;

            string strHistorycontent = MakeHistoryContentFakeWall(targetType, actionType, ModifyType.None , zoneID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 현황정보 업로드/다운로드
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="isUpload">true:업로드</param>
        /// <returns></returns>
        public bool SaveUserHistory_StatusInfo(int userID, bool isUpload)
        {
            TargetType targetType = TargetType.StatusInfo;
            ActionType actionType = (isUpload) ? ActionType.Upload : ActionType.Download;

            string strHistorycontent = MakeHistoryContentStatusInfo(targetType, actionType);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// 사용자 권한 수정
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="memberID"></param>
        /// <param name="orgLevel">수정하기 전 권한 ID, 권한이 없었다면 -1</param>
        /// <returns></returns>
        public bool SaveUserHistory_ModifyUserAuth(int userID, int memberID, int orgLevel)
        {
            TargetType targetType = TargetType.UserAuth;
            ActionType actionType = ActionType.Modify;

            string strHistorycontent = MakeHistoryContentUserAuth(targetType, actionType, memberID, orgLevel);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// SOP 추가
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nDisasterCategoryID"></param>
        /// <param name="nSubDisasterCategoryID"></param>
        /// <param name="nDisasterID"></param>
        /// <param name="nVersionID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_AddSop(int userID, int nDisasterCategoryID, int nSubDisasterCategoryID, int nDisasterID, int nVersionID)
        {
            TargetType targetType = TargetType.SOP;
            ActionType actionType = ActionType.Add;

            string strHistorycontent = MakeHistoryContentSop(targetType, actionType, nDisasterCategoryID, nSubDisasterCategoryID, nDisasterID, nVersionID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// SOP 수정
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nDisasterCategoryID"></param>
        /// <param name="nSubDisasterCategoryID"></param>
        /// <param name="nDisasterID"></param>
        /// <param name="nVersionID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_ModifySop(int userID, int nDisasterCategoryID, int nSubDisasterCategoryID, int nDisasterID, int nVersionID)
        {
            TargetType targetType = TargetType.SOP;
            ActionType actionType = ActionType.Add;

            string strHistorycontent = MakeHistoryContentSop(targetType, actionType, nDisasterCategoryID, nSubDisasterCategoryID, nDisasterID, nVersionID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        /// <summary>
        /// SOP 삭제
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="nDisasterCategoryID"></param>
        /// <param name="nSubDisasterCategoryID"></param>
        /// <param name="nDisasterID"></param>
        /// <param name="nVersionID"></param>
        /// <returns></returns>
        public bool SaveUserHistory_DeleteSopManager(int userID, int nDisasterCategoryID, int nSubDisasterCategoryID, int nDisasterID, int nVersionID)
        {
            TargetType targetType = TargetType.SOP;
            ActionType actionType = ActionType.Add;

            string strHistorycontent = MakeHistoryContentSop(targetType, actionType, nDisasterCategoryID, nSubDisasterCategoryID, nDisasterID, nVersionID);

            bool result = m_dataManager.GetCreateManager().CreateUserHistory(userID, (int)targetType, (int)actionType, strHistorycontent);
            return result;
        }

        private string MakeHistoryContentPOI(TargetType targetType, ActionType actionType, int nZoneID, int nFacilityType, int nSensorOrgID)
        {
            SDMS.IDAL.ISelect sdmsSelect = m_processManager.SdmsDataManager.GetSelectManager();

            string position = GetTargetPosition(sdmsSelect, nZoneID);
            string strFacilityType = dnsData.Sensor.Facility.GetNFacilityTypeString(nFacilityType);
            string sensorName = "";

            dnsData.Sensor.Facility.FacilityType facilityType = dnsData.Sensor.Facility.ToFacilityType(nFacilityType);
            if (facilityType == dnsData.Sensor.Facility.FacilityType.FIRE_SENSOR)
            {
                string strErrorMessage = null;
                SDMS.Model.Sensor.Fire fire = sdmsSelect.SelectFireSensor(nSensorOrgID, out strErrorMessage);
                if (fire != null)
                    sensorName = fire.Name;
            }
            else if (facilityType == dnsData.Sensor.Facility.FacilityType.PSM_SENSOR)
            {
                string strErrorMessage = null;
                SDMS.Model.Sensor.PSM psm = sdmsSelect.SelectPSMSensor(nSensorOrgID, out strErrorMessage);
                if (psm != null)
                    sensorName = psm.Name;
            }
            else if (facilityType == dnsData.Sensor.Facility.FacilityType.ETC)
            {
                string strErrorMessage = null;
                SDMS.Model.Sensor.ETC etc = sdmsSelect.SelectETCSensor(nSensorOrgID, out strErrorMessage);
                if (etc != null)
                    sensorName = etc.Name;
            }
            else if (facilityType == dnsData.Sensor.Facility.FacilityType.CCTV)
            {
                string strErrorMessage = null;
                CCTV cctv = sdmsSelect.SelectCCTV(nSensorOrgID, out strErrorMessage);
                if (cctv != null)
                    sensorName = cctv.CameraName;
            }

            string strType = GetTypeString(targetType, actionType);

            string returnStr = string.Format("[{0}] {1} [{2}] {3}", position, strFacilityType, sensorName, strType);

            return returnStr;
        }

        private string MakeHistoryContentEquipzoneName(TargetType targetType, ActionType actionType, int nZoneID, string orgEquipzoneName, int nEquipzoneID)
        {
            SDMS.IDAL.ISelect sdmsSelect = m_processManager.SdmsDataManager.GetSelectManager();

            string position = GetTargetPosition(sdmsSelect, nZoneID);

            string equipzoneName = "";
            string strErrorMessage = null;

            EquipmentZone equipmentZone = sdmsSelect.SelectEquipmentZone(nEquipzoneID, out strErrorMessage);
            if (equipmentZone != null)
                equipzoneName = equipmentZone.ZoneName;

            string strType = GetTypeString(targetType, actionType);

            string returnStr = "";
            if (actionType == ActionType.Move)
                returnStr = string.Format("[{0}] [{1}] {2}", position, equipzoneName, strType);
            else if (actionType == ActionType.Modify)
                returnStr = string.Format("[{0}] [{1} > {2}] {3}", position, orgEquipzoneName, equipzoneName, strType);

            return returnStr;
        }

        private string MakeHistoryContentBuildingGroupName(TargetType targetType, ActionType actionType, int nBuildingGroupID, string orgBuildingGroupName)
        {
            SDMS.IDAL.ISelect sdmsSelect = m_processManager.SdmsDataManager.GetSelectManager();

            string buildingGroupName = "";
            string strErrorMessage = null;

            BuildingGroup buildingGroup = sdmsSelect.SelectBuildingGroup(nBuildingGroupID, out strErrorMessage);
            if (buildingGroup != null)
                buildingGroupName = buildingGroup.DisplayText;

            string strType = GetTypeString(targetType, actionType);

            string returnStr = "";
            if (actionType == ActionType.Move)
                returnStr = string.Format("[{0}] {1}", buildingGroupName, strType);
            else if (actionType == ActionType.Modify)
                returnStr = string.Format("[{0} > {1}] {2}", orgBuildingGroupName, buildingGroupName, strType);

            return returnStr;
        }

        private string MakeHistoryContentBuildingName(TargetType targetType, ActionType actionType, int nBuildingID, string orgBuildingName)
        {
            SDMS.IDAL.ISelect sdmsSelect = m_processManager.SdmsDataManager.GetSelectManager();

            string buildingName = "";
            string strErrorMessage = null;

            Building building = sdmsSelect.SelectBuilding(nBuildingID, out strErrorMessage);
            if (building != null)
                buildingName = building.DisplayText;

            string strType = GetTypeString(targetType, actionType);

            string returnStr = "";
            if (actionType == ActionType.Move)
                returnStr = string.Format("[{0}] {1}", buildingName, strType);
            else if (actionType == ActionType.Modify)
                returnStr = string.Format("[{0} > {1}] {2}", orgBuildingName, buildingName, strType);

            return returnStr;
        }

        private string MakeHistoryContentFakeWall(TargetType targetType, ActionType actionType, ModifyType modifyType, int nZoneID)
        {
            SDMS.IDAL.ISelect sdmsSelect = m_processManager.SdmsDataManager.GetSelectManager();

            string position = GetTargetPosition(sdmsSelect, nZoneID);
            string strType = GetTypeString(targetType, actionType, modifyType);

            string returnStr = string.Format("[{0}] {1}", position, strType);

            return returnStr;
        }

        private string MakeHistoryContentSop(TargetType targetType, ActionType actionType, int nDisasterCategoryID, int nSubDisasterCategoryID, int nDisasterID, int nVersionID)
        {
            //string strType = GetTypeString(targetType, actionType);

            //string returnStr = string.Format("[{0} > {1} > {2}] {3}", position, strType);

            return "";
        }


        private string MakeHistoryContentStatusInfo(TargetType targetType, ActionType actionType)
        {
            string strType = GetTypeString(targetType, actionType);
            string returnStr = string.Format("{0}", strType);

            return returnStr;
        }

        private string MakeHistoryContentUserAuth(TargetType targetType, ActionType actionType, int memberID, int orgLevel)
        {
            // 권한없음 > 관리자 orgLevel=-1
            // 관리자 > 권한없음 orgLevel= 1
            // 사용자 > 관리자   orgLevel=1
            string strUserName = "";
            string strOrgLevel = "";
            string strChgLevel = "";

            string strErrorMessage = null;
            Dictionary<User.Fields, object> dicCondition = new Dictionary<User.Fields, object>();
            dicCondition.Add(User.Fields.MemberID, memberID);

            List<User> user = m_processManager.SopDataManager.GetSelectManager().SelectUsers(dicCondition, out strErrorMessage);
            if (user == null)
                return "";

            // 권한이 없으면 User 테이블에 없음

            // 권한 있음 > 권한 없음으로 변경
            if (user.Count == 0)
            {
                strChgLevel = "권한 없음";
                RegularMember member = m_processManager.TeamDataManager.GetSelectManager().SelectRegularMember(memberID, out strErrorMessage);
                if (member == null)
                    return "";

                strUserName = member.MemberName;

                Level level = m_processManager.SopDataManager.GetSelectManager().SelectLevel(orgLevel, out strErrorMessage);
                if (level == null)
                    return "";

                strOrgLevel = level.LevelName;
            }
            else
            {
                strUserName = user[0].NickName;
                int nChgLevel = user[0].UserLevel;

                Level level = m_processManager.SopDataManager.GetSelectManager().SelectLevel(nChgLevel, out strErrorMessage);
                if (level == null)
                    return "";

                strChgLevel = level.LevelName;

                if (orgLevel >= 0)
                {
                    level = m_processManager.SopDataManager.GetSelectManager().SelectLevel(orgLevel, out strErrorMessage);
                    if (level == null)
                        return "";

                    strOrgLevel = level.LevelName;
                }
                else
                    strOrgLevel = "권한 없음";
            }

            string strType = GetTypeString(targetType, actionType);
            string returnStr = string.Format("[{0}] [{1} > {2}] {3}", strUserName, strOrgLevel, strChgLevel, strType);

            return returnStr;
        }

        private string GetTargetPosition(SDMS.IDAL.ISelect sdmsSelect, int nZoneID)
        {
            string strErrorMessage = null;

            Dictionary<SDMS.Model.Spatial.Zone.Fields, object> dicCondition = new Dictionary<SDMS.Model.Spatial.Zone.Fields, object>();
            dicCondition.Add(SDMS.Model.Spatial.Zone.Fields.ID, nZoneID);

            if (nZoneID < 10000)
            {
                ArrayList arrResult = sdmsSelect.JoinBuildingGroupBuildingZone(null, null, dicCondition, null, out strErrorMessage);
                if (arrResult == null)
                    return "";

                if (arrResult.Count == 3 && arrResult[0] is BuildingGroup && arrResult[1] is Building && arrResult[2] is Zone)
                {
                    BuildingGroup buildingGroup = arrResult[0] as BuildingGroup;
                    Building building = arrResult[1] as Building;
                    Zone zone = arrResult[2] as Zone;

                    string position = string.Format("{0} {1} {2}", buildingGroup.GroupName, building.BuildingName, zone.ZoneName);
                    return position;
                } 
            }            
            else
            {
                Zone zone = sdmsSelect.SelectZone(nZoneID, out strErrorMessage);
                if (zone == null)
                    return "";

                return zone.ZoneName;
            }

            return "";
        }

        private string GetTypeString(TargetType targetType, ActionType actionType, ModifyType modifyType = ModifyType.None)
        {            
            string returnStr = "";
            if (targetType == TargetType.POI)
                returnStr = "POI";
            else if (targetType == TargetType.EquipzoneName)
                returnStr = "공간정보";
            else if (targetType == TargetType.FakeWall)
                returnStr = "가벽";
            else if (targetType == TargetType.SOP)
                returnStr = "SOP";
            else if (targetType == TargetType.StatusInfo)
                returnStr = "현황정보";
            else if (targetType == TargetType.UserAuth)
                returnStr = "사용자 권한";
            else if (targetType == TargetType.BuildingGroupName)
                returnStr = "빌딩 그룹 명칭";
            else if (targetType == TargetType.BuildingName)
                returnStr = "빌딩 명칭";

            if (actionType == ActionType.Add)
                returnStr += " 추가";
            else if (actionType == ActionType.Modify)
            {
                if (targetType == TargetType.EquipzoneName || targetType == TargetType.BuildingGroupName || targetType == TargetType.BuildingName)
                    returnStr += " 명칭 변경";
                else if (targetType == TargetType.FakeWall)
                {
                    if (modifyType == ModifyType.ChangeSize)
                        returnStr += " 크기 변경";
                    else if (modifyType == ModifyType.Move)
                        returnStr += " 이동";
                    else if (modifyType == ModifyType.Rotate)
                        returnStr += " 회전";
                    else
                        returnStr += " 수정";
                }
                else
                    returnStr += " 수정";
            }
            else if (actionType == ActionType.Move)
                returnStr += " 위치 이동";
            else if (actionType == ActionType.Delete)
                returnStr += " 삭제";
            else if (actionType == ActionType.Upload)
                returnStr += " 업로드";
            else if (actionType == ActionType.Download)
                returnStr += " 다운로드";

            return returnStr;
        }


        public MessageResult SaveSOPSetting(RequestSaveSetting req)
        {
            MessageResult result = new MessageResult();

            string strErrorMessage = null;
            //req.PropertyName
            List<Common.Model.Option.Options> options = m_processManager.CommonDataManager.GetSelectManager().SelectOption(Model.Option.Options.OptionTarget.SOPSimulator, req.PropertyName, out strErrorMessage);
            if (options == null)
            {
                result.Success = false;
                result.Message = "SelectOption 실패";
                return result;
            }

            if (options.Count == 0)
            {
                Common.Model.Option.Options option = m_processManager.CommonDataManager.GetCreateManager().CreateOption(Model.Option.Options.OptionTarget.SOPSimulator, req.PropertyName, req.PropertyValue, m_dataManager.SiteID);
                if (option == null)
                {
                    result.Success = false;
                    result.Message = "CreateOption 실패";
                    return result;
                }
            }
            else
            {
                Common.Model.Option.Options option = options[0];
                option.PropertyValue = req.PropertyValue;
                if (!m_processManager.CommonDataManager.GetUpdateManager().UpdateOption(Model.Option.Options.OptionTarget.SOPSimulator, option))
                {
                    result.Success = false;
                    result.Message = "UpdateOption 실패";
                    return result;
                }
            }

            result.Success = true;
            return result;
        }
    }
}
