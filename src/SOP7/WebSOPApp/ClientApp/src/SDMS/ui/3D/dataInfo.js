import SdmsResource from "../../resource/id";
import { SDMSController } from "../../services/sdmsController";
import { SDMSDataManager } from "../../services/sdmsDataManager";

export class DataInfo {
    static async processFacilityInfo(modelName/*: string*/, showBuildingInfoMethod) {
        if (modelName === null) {
            showBuildingInfoMethod("", null);
            return;
        }

        const response = await SDMSController.requestFacilityInfoData(modelName);

        if (response === null) {
            alert(SdmsResource.ID.errorMessage.loadFailFacilityInfo);
        }
        else if (response.success === false) {
            alert(response.message);
        }
        else {
            const datas = [];
            const dataCount = response.datas.length;

            for (let i = 0; i < dataCount; i++) {
                const data = response.datas[i];
                datas.push([data.value, data.withDot, data.indentDepth]);
            }

            const arrInfo = new Array();

            arrInfo[0] = SdmsResource.ID.buildingInfo.equipmentType;       // 설비
            arrInfo[1] = response.facilityName;                            // 설비 이름
            arrInfo[2] = datas;

            showBuildingInfoMethod(arrInfo[0], arrInfo);
        }
    }

    static async processBuildingData(modelName/*: string*/, showBuildingInfoMethod) {
        const buildingName = DataInfo.getBuildingName(modelName);
        const response = await SDMSController.requestBuildingData(buildingName);

        if (response === null) {
            alert(SdmsResource.ID.errorMessage.loadFailBuildingData);
        }
        else if (response.success === false) {
            alert(response.message);
        }
        else {
            const datas = [];
            const dataCount = response.datas.length;

            for (let i = 0; i < dataCount; i++) {
                const data = response.datas[i];
                datas.push([data.value, data.withDot, data.indentDepth]);
            }

            const arrInfo = new Array();

            arrInfo[0] = SdmsResource.ID.buildingInfo.buildingType;        // 건물
            arrInfo[1] = response.displayText;                             // 건물 이름
            arrInfo[2] = datas;

            showBuildingInfoMethod(arrInfo[0], arrInfo);
        }
    }

    static async processBuildingGroupData(buildingGroupID/*: number*/, showBuildingInfoMethod) {
        const response = await SDMSController.requestBuildingGroupData(buildingGroupID);

        if (response === null) {
            alert(SdmsResource.ID.errorMessage.loadFailBuildingGroupData);
        }
        else if (response.success === false) {
            alert(response.message);
        }
        else {
            const datas = [];
            const dataCount = response.datas.length;

            for (let i = 0; i < dataCount; i++) {
                const data = response.datas[i];
                datas.push([data.value, data.withDot, data.indentDepth]);
            }

            const arrInfo = new Array();

            arrInfo[0] = SdmsResource.ID.buildingInfo.buildingGroupType;   // 건물
            arrInfo[1] = response.displayText;                             // 건물 이름
            arrInfo[2] = datas;

            showBuildingInfoMethod(arrInfo[0], arrInfo);
        }
    }

    static getBuildingName(boundingBoxName) {
        return boundingBoxName.substring(0, boundingBoxName.length - SDMSDataManager.BoundingBoxTag.length);
    }
}